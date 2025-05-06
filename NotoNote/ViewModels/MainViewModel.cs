using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotoNote.Models;
using NotoNote.Services;
using MessageBox = System.Windows.MessageBox;

namespace NotoNote.ViewModels;
public partial class MainViewModel(
    IAudioService audioService,
    ITranscriptionAiServiceLocator transcription,
    IChatAiServiceLocator chat) : ObservableObject
{
    private readonly IAudioService _audioService = audioService;
    private readonly ITranscriptionAiServiceLocator _transcription = transcription;
    private readonly IChatAiServiceLocator _chat = chat;

    private readonly string _systemPrompt =
"""
あなたは日記向けに音声書き起こしを整理するアシスタントです。
- 出力は日本語。
- すべて「-」の箇条書き。
- 1 行 1 トピック、30 行以内に要約。
- 事実を改変しない。情報が重複する場合は統合する。
- 日付・数値・人物名などの固有情報は省略せず正確に残す。
- 敬語／口語混在は整え、表記ゆれを修正する。
- もとの発話内容にないことは書かない。
以下の発話内容を上記ルールで箇条書きにまとめてください。
---
""";

    [ObservableProperty] private string _status = "Idle";
    [ObservableProperty] private string _resultText = "";

    [RelayCommand]
    private async Task ToggleRecording()
    {
        try
        {
            switch (Status)
            {
                case "Idle":
                    _audioService.StartRecording();
                    Status = "Recording...";
                    break;
                case "Recording...":
                    Status = "Processing...";

                    // 1. record
                    var audioFilePath = await _audioService.StopRecordingAsync();

                    // 2. transcribe
                    var transcriptionModel = AiModelConsts.TranscriptionAiModels[0];
                    var transcriptionService = _transcription.GetService(transcriptionModel);
                    var transcript = await transcriptionService.TranscribeAsync(audioFilePath);

                    // 4. chat
                    var chatAiModel = AiModelConsts.ChatAiModels[0];
                    var chatAiService = _chat.GetService(chatAiModel);
                    var chatResponse = await chatAiService.CompleteChatAsync(_systemPrompt, transcript);

                    // 5. paste
                    ResultText = chatResponse;
                    ClipBoardService.Paste(ResultText);
                    Status = "Idle";
                    break;
            }
        }
        catch (Exception e)
        {
            Status = "Error";
            MessageBox.Show(e.Message, "noto note");
        }
    }
}
