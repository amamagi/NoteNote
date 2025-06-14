﻿using NotoNote.Models;

namespace NotoNote.Services;
public sealed class PresetProfileProvider : IPresetProfileProvider
{
    private readonly List<(ProfileId Id, ProfileName Name, SystemPrompt SystemPrompt)> _presetProfileSettings =
    [
        (
            new(Guid.Parse("4af62f93-3bcb-4bc6-a530-ee298d6df703")),
            new("書き起こし"),
            new(
"""  
あなたは音声の書き起こしを整形するアシスタントです。以下のルールに従って書き起こしを整形してください。
- 出力は日本語、ただし固有名詞には英語も使える。状況によって使い分ける。
- 言い間違いや言い直しは削除する
- 同じような文節が続く場合は、1 つにまとめる
- 文の順序は論理構造が成立するように並べ替える
- 横に長くなりすぎないよう適度に改行する
- 事実を改変しない。情報が重複する場合は統合する
- 日付・数値・人物名などの固有情報は省略せず正確に残す
- 敬語／口語混在は整え、表記ゆれを修正する
---
"""
        )
    ),
    (
        new(Guid.Parse("2b7c008d-5d60-4e12-9364-da13e1a77f02")),
        new("要約"),
        new(
"""
あなたは音声の書き起こしを整形するアシスタントです。以下のルールに従って書き起こしを整形してください。",
- 出力は日本語、ただし固有名詞には英語も使える。状況によって使い分ける。",
- すべて「-」の箇条書き",
- 発言の内容を構造化して箇条書きのインデントに反映させる",
- 1 行 1 トピック、50 行以内に要約",
- 入力の文字数に合わせて要約の文字数も増減させる調整する",
- 事実を改変しない。情報が重複する場合は統合する",
- 日付・数値・人物名などの固有情報は省略せず正確に残す",
- 敬語／口語混在は整え、表記ゆれを修正する",
- 締めの言葉は不要",
---
"""
            )
        ),
        (
            new(Guid.Parse("d1c5f0b2-3e4a-4b8c-9f6d-7e8f9a0b1c2d")),
            new("メッセージ"),
            new(
 """
あなたは音声の書き起こしを整形するアシスタントです。以下のルールに従って書き起こしを整形してください。",
- 口語的な表現を読みやすいメッセージ文にする。
- 語尾は「ですます調」で統一。
- 原文の意味やニュアンスは忠実に保持すること。
- ですます調の丁寧な文体に統一すること。
- 不要な省略やスラングは避け、分かりやすい言葉に置き換えること。
- 適宜句読点を補い、読みやすい長さの文に分割すること。
---
"""
            )
        ),
        (
            new(Guid.Parse("044badc8-724b-4ca7-a890-a2e23b9c3412")),
            new("英訳"),
            new(
"""
あなたは音声の書き起こしを英語に翻訳するアシスタントです。以下のルールに従って書き起こしを整形してください。
- 不要な省略やスラングは避け、分かりやすい言葉に置き換えること。
- 原文の意味やニュアンスは忠実に保持すること。
---
"""

        ))
    ];

    private readonly List<Profile> _profiles = [];

    public PresetProfileProvider()
    {
        var defaultTranscriptionModel = PresetModelProvider.DefaultTranscriptionModel;
        var defaultChatModel = PresetModelProvider.DefaultChatModel;

        foreach (var profileSetting in _presetProfileSettings)
        {
            _profiles.Add(new Profile(
                profileSetting.Id,
                profileSetting.Name,
                profileSetting.SystemPrompt,
                defaultTranscriptionModel,
                defaultChatModel));
        }
    }

    public List<Profile> Get() => _profiles;
}