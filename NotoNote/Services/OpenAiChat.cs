using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace NotoNote.Services;

public sealed class OpenAiChat : ILanguageProcessingService
{
    private readonly ChatClient _client;
    private readonly string _systemPrompt =
"""
あなたは議事メモを作成するアシスタントです。
- 出力は日本語。
- すべて「・」の箇条書き。
- 1 行 1 トピック、10 行以内に要約。
- 文末は「です／ます」を避け、体言止めまたは簡潔な述語で。
- 事実を改変しない。情報が重複する場合は統合する。
- 日付・数値・人物名などの固有情報は省略せず正確に残す。
- 敬語／口語混在は整え、表記ゆれを修正する。
以下の発話内容を上記ルールで箇条書きにまとめてください。
---
""";

    public OpenAiChat(IOptions<OpenAiOptions> options)
    {
        var apiKey = options.Value.ApiKey;
        var model = options.Value.LanguageModel;
        _client = new ChatClient(model, apiKey);
    }

    public async Task<string> ProcessTranscriptAsync(string transcript)
    {
        var completion = await _client.CompleteChatAsync(_systemPrompt, transcript);
        return completion.Value.Content[0].Text;
    }
}
