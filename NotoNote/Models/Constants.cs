namespace NotoNote.Models;

public static class Constants
{
    public static readonly IReadOnlyList<ITranscriptionAiModel> AvailableTranscriptionAiModels =
        [
            new OpenAiTranscribeAiModel(
                new TranscriptionAiModelId("openai-whisper-1"), 
                new AiModelName("Whisper"), 
                new OpenAiApiId("whisper-1"))
        ];

    public static readonly IReadOnlyList<IChatAiModel> AvailableChatAiModels =
        [
            new OpenAiChatAiModel(
                new ChatAiModelId("openai-gpt-4o-mini"),
                new AiModelName("ChatGPT 4o-mini"),
                new OpenAiApiId("gpt-4o-mini"))
        ];

    public static readonly IReadOnlyList<Profile> SampleProfiles = [
        new (
            new ProfileName("Summary"),
            new SystemPrompt("""
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
            """),
            AvailableTranscriptionAiModels!.FirstOrDefault() ?? throw new ArgumentException(),
            AvailableChatAiModels!.FirstOrDefault() ?? throw new ArgumentException()),
        new (
            new ProfileName("Message"),
            new SystemPrompt("""
            あなたは Slack チャンネルに書き込むメッセージを整形するアシスタントです。  
            以下のルールでユーザーの発話を 1 本の Slack 文章にまとめてください。

            1. 出力は日本語、敬語とフランクの混在を整え、口語の軽さを残す。  
            2. 先頭に :memo: の絵文字と半角スペースを置く。  
            3. 箇条書きは `•` (U+2022) を使い、最大 6 行。  
               - 行頭以外の絵文字・記号は控える。  
            4. 太字は強調語のみ `*bold*`、URL やコードは ```バッククォート```。  
            5. 1 行は 80 文字以内で改行。  
            6. 事実を改変せず、できるだけ短く。  
            7. フッターに自動タグ `#noto-note` を付与する。
            ---
            """),
            AvailableTranscriptionAiModels!.FirstOrDefault() ?? throw new ArgumentException(),
            AvailableChatAiModels!.FirstOrDefault() ?? throw new ArgumentException())
        ];
}
