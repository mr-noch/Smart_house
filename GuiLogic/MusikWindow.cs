using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using SmartHouseUI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;

namespace SmartHouseUI.GuiLogic;

public partial class MusikPanel : UserControl
{
    private readonly ObservableCollection<MusicTrack> _musicFiles = new();
    private Process? _audioProcess;
    private ListBox? _musicListBox;
    private TextBlock? _currentTrackText;
    private TextBlock? _statusText;
    private Slider? _musicSlider;
    private string? _currentFile;
    private double _currentTrackDuration;
    private Timer? _progressTimer;
    private bool _isDraggingSlider;

    public MusikPanel()
    {
        InitializeComponent();
        AttachControls();
    }

    private void AttachControls()
    {
        _musicListBox = this.FindControl<ListBox>("MusicList");
        _currentTrackText = this.FindControl<TextBlock>("CurrentTrackText");
        _statusText = this.FindControl<TextBlock>("StatusText");
        _musicSlider = this.FindControl<Slider>("MusicSlider");

        if (_musicListBox != null)
        {
            _musicListBox.ItemsSource = _musicFiles;
        }

        if (_musicSlider != null)
        {
            _musicSlider.Minimum = 0;
            _musicSlider.Maximum = 100;
            _musicSlider.Value = 0;
            _musicSlider.PointerPressed += (_, _) => _isDraggingSlider = true;
            _musicSlider.PointerReleased += (_, _) =>
            {
                _isDraggingSlider = false;
                SeekToSliderPosition();
            };
            _musicSlider.PropertyChanged += (sender, args) =>
            {
                if (args.Property == Slider.ValueProperty && _isDraggingSlider)
                {
                    UpdateTimeText(_musicSlider.Value, _currentTrackDuration);
                }
            };
        }

        LoadUserMusic();
        UpdateStatus("Очікує");
        UpdateCurrentTrack(null);
    }

    private void UpdateStatus(string status)
    {
        if (_statusText is not null)
        {
            _statusText.Text = status;
        }
    }

    private void UpdateCurrentTrack(string? filePath)
    {
        if (_currentTrackText is not null)
        {
            _currentTrackText.Text = string.IsNullOrWhiteSpace(filePath) ? "Немає треку" : Path.GetFileName(filePath);
        }
    }

    private async void AddMusicButton_Click(object? sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Виберіть аудіофайл",
            AllowMultiple = true,
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "Аудіо файли", Extensions = { "mp3", "wav", "ogg", "flac", "aac", "m4a" } },
                new FileDialogFilter { Name = "Усі файли", Extensions = { "*" } }
            }
        };

        var parentWindow = this.GetVisualRoot() as Window;
        var result = await dialog.ShowAsync(parentWindow!);
        if (result is null || result.Length == 0)
        {
            return;
        }

        foreach (var file in result)
        {
            if (!_musicFiles.Any(track => string.Equals(track.Path, file, StringComparison.OrdinalIgnoreCase)))
            {
                _musicFiles.Add(new MusicTrack(file));
            }
        }

        if (_musicFiles.Count > 0)
        {
            _musicListBox!.SelectedIndex = 0;
        }

        SaveUserMusic();
        UpdateStatus("Треки додано");
    }

    private void PlayButton_Click(object? sender, RoutedEventArgs e)
    {
        var track = _musicListBox?.SelectedItem as MusicTrack ?? _musicFiles.FirstOrDefault();
        if (track == null || string.IsNullOrWhiteSpace(track.Path))
        {
            UpdateStatus("Оберіть трек для відтворення");
            return;
        }

        _currentFile = track.Path;
        UpdateCurrentTrack(track.Path);
        _currentTrackDuration = GetTrackDuration(track.Path);

        if (_musicSlider != null)
        {
            _musicSlider.Maximum = _currentTrackDuration > 0 ? _currentTrackDuration : 100;
            _musicSlider.Value = 0;
        }

        UpdateTimeText(0, _currentTrackDuration);

        if (PlayMusicAtPosition(track.Path, 0))
        {
            UpdateStatus("Відтворюється");
            StartProgressTimer();
        }
        else
        {
            UpdateStatus("Неможливо відтворити файл");
        }
    }

    private void StopButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_audioProcess is not null && !_audioProcess.HasExited)
        {
            try
            {
                _audioProcess.Kill(true);
                _audioProcess.Dispose();
            }
            catch
            {
                // Ігнорувати помилки зупинки.
            }
            finally
            {
                _audioProcess = null;
            }

            StopProgressTimer();
            UpdateStatus("Відтворення зупинено");
            return;
        }

        UpdateStatus("Немає активного відтворення");
    }

    private void DeleteMusicButton_Click(object? sender, RoutedEventArgs e)
    {
        DeleteSelectedTrack();
    }

    private void ClosePanel_Click(object? sender, RoutedEventArgs e)
    {
        StopProgressTimer();
        this.IsVisible = false;
    }

    private bool PlayMusic(string filePath)
    {
        StopCurrentAudio();

        if (TryPlayWithFfplay(filePath, 0))
        {
            return true;
        }

        return TryOpenFileWithSystemPlayer(filePath);
    }

    private bool PlayMusicAtPosition(string filePath, double positionSeconds)
    {
        StopCurrentAudio();
        StopProgressTimer();

        if (_musicSlider != null)
        {
            _musicSlider.Value = positionSeconds;
        }

        if (TryPlayWithFfplay(filePath, positionSeconds))
        {
            StartProgressTimer();
            return true;
        }

        return TryOpenFileWithSystemPlayer(filePath);
    }

    private void StartProgressTimer()
    {
        StopProgressTimer();

        if (_musicSlider == null || _currentTrackDuration <= 0)
        {
            return;
        }

        _progressTimer = new Timer(500);
        _progressTimer.Elapsed += (sender, args) => Dispatcher.UIThread.Post(UpdateProgress);
        _progressTimer.Start();
    }

    private void StopProgressTimer()
    {
        if (_progressTimer is not null)
        {
            _progressTimer.Stop();
            _progressTimer.Dispose();
            _progressTimer = null;
        }
    }

    private void UpdateProgress()
    {
        if (_musicSlider is null || _currentTrackDuration <= 0 || _isDraggingSlider)
        {
            return;
        }

        if (_audioProcess is null || _audioProcess.HasExited)
        {
            StopProgressTimer();
            return;
        }

        var nextValue = Math.Min(_musicSlider.Value + 0.5, _musicSlider.Maximum);
        _musicSlider.Value = nextValue;
        UpdateTimeText(nextValue, _currentTrackDuration);
    }

    private void SeekToSliderPosition()
    {
        if (_currentFile is null || _currentTrackDuration <= 0 || _musicSlider is null)
        {
            return;
        }

        var newPosition = _musicSlider.Value;
        if (newPosition < 0 || newPosition > _musicSlider.Maximum)
        {
            return;
        }

        PlayMusicAtPosition(_currentFile, newPosition);
    }

    private void UpdateTimeText(double currentSeconds, double totalSeconds)
    {
        if (_currentTrackText is null)
        {
            return;
        }

        var currentTime = TimeSpan.FromSeconds(currentSeconds);
        var totalTime = TimeSpan.FromSeconds(totalSeconds);

        var currentText = currentTime.ToString("mm\\:ss");
        var totalText = totalSeconds > 0 ? totalTime.ToString("mm\\:ss") : "00:00";

        var currentTimeBlock = this.FindControl<TextBlock>("CurrentTimeText");
        var totalTimeBlock = this.FindControl<TextBlock>("TotalTimeText");

        if (currentTimeBlock != null)
        {
            currentTimeBlock.Text = currentText;
        }

        if (totalTimeBlock != null)
        {
            totalTimeBlock.Text = totalText;
        }
    }

    private double GetTrackDuration(string filePath)
    {
        if (!CommandExists("ffprobe"))
        {
            return 0;
        }

        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "ffprobe",
                Arguments = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{filePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var probe = Process.Start(startInfo);
            if (probe == null)
            {
                return 0;
            }

            var output = probe.StandardOutput.ReadToEnd();
            probe.WaitForExit();

            if (double.TryParse(output.Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var duration))
            {
                return duration;
            }
        }
        catch
        {
            // ignore
        }

        return 0;
    }

    private bool TryPlayWithFfplay(string filePath, double startPosition)
    {
        if (!CommandExists("ffplay"))
        {
            return false;
        }

        try
        {
            var arguments = $"-nodisp -autoexit -loglevel quiet";
            if (startPosition > 0)
            {
                arguments += $" -ss {startPosition}";
            }
            arguments += $" \"{filePath}\"";

            var startInfo = new ProcessStartInfo
            {
                FileName = "ffplay",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            _audioProcess = Process.Start(startInfo);
            return _audioProcess is not null;
        }
        catch
        {
            _audioProcess = null;
            return false;
        }
    }

    private bool TryOpenFileWithSystemPlayer(string filePath)
    {
        try
        {
            ProcessStartInfo startInfo;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                startInfo = new ProcessStartInfo(filePath)
                {
                    UseShellExecute = true
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                startInfo = new ProcessStartInfo("open", $"\"{filePath}\"")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }
            else
            {
                startInfo = new ProcessStartInfo("xdg-open", $"\"{filePath}\"")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }

            _audioProcess = Process.Start(startInfo);
            return true;
        }
        catch
        {
            _audioProcess = null;
            return false;
        }
    }

    private void StopCurrentAudio()
    {
        if (_audioProcess is null)
        {
            return;
        }

        try
        {
            if (!_audioProcess.HasExited)
            {
                _audioProcess.Kill(true);
            }
        }
        catch
        {
            // Ігнорувати помилки зупинки.
        }
        finally
        {
            _audioProcess.Dispose();
            _audioProcess = null;
        }
    }

    private void LoadUserMusic()
    {
        var currentUser = UserSession.CurrentUser;
        if (currentUser != null && currentUser.MusicFiles != null)
        {
            _musicFiles.Clear();
            foreach (var file in currentUser.MusicFiles)
            {
                if (File.Exists(file))
                {
                    _musicFiles.Add(new MusicTrack(file));
                }
            }
        }
    }

    private void SaveUserMusic()
    {
        var currentUser = UserSession.CurrentUser;
        if (currentUser != null)
        {
            currentUser.MusicFiles = _musicFiles.Select(track => track.Path).ToList();
            UserAuthService.SaveAllUsers();
        }
    }

    private void DeleteSelectedTrack()
    {
        var track = _musicListBox?.SelectedItem as MusicTrack;
        if (track == null)
        {
            UpdateStatus("Оберіть трек для видалення");
            return;
        }

        if (string.Equals(track.Path, _currentFile, StringComparison.OrdinalIgnoreCase))
        {
            StopCurrentAudio();
            StopProgressTimer();
            _currentFile = null;
            UpdateTimeText(0, 0);
            UpdateCurrentTrack(null);
        }

        _musicFiles.Remove(track);
        SaveUserMusic();
        UpdateStatus("Трек видалено");

        if (_musicFiles.Count > 0)
        {
            _musicListBox!.SelectedIndex = 0;
        }
    }

    private sealed class MusicTrack
    {
        public string Path { get; }
        public string Name => System.IO.Path.GetFileName(Path);

        public MusicTrack(string path)
        {
            Path = path;
        }

        public override string ToString() => Name;
    }

    private bool CommandExists(string command)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "where" : "which",
                Arguments = command,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process != null)
            {
                process.WaitForExit();
                return process.ExitCode == 0;
            }
        }
        catch
        {
            // ignore
        }

        return false;
    }
}
