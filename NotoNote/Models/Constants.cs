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

    public static readonly Dictionary<TranscriptionAiModelId, ITranscriptionAiModel> TranscriptionAiModelMap
        = AvailableTranscriptionAiModels.ToDictionary(model => model.Id, model => model);

    public static readonly Dictionary<ChatAiModelId, IChatAiModel> ChatAiModelMap
        = AvailableChatAiModels.ToDictionary(model => model.Id, model => model);

    public static readonly Keys[] AvailableKeys =
        {
            // Letters
            Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H,
            Keys.I, Keys.J, Keys.K, Keys.L, Keys.M, Keys.N, Keys.O, Keys.P,
            Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X,
            Keys.Y, Keys.Z,

            // Numbers (top row)
            Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4,
            Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9,

            // Numpad
            Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3,
            Keys.NumPad4, Keys.NumPad5, Keys.NumPad6,
            Keys.NumPad7, Keys.NumPad8, Keys.NumPad9,
            Keys.Multiply, Keys.Add, Keys.Subtract, Keys.Decimal, Keys.Divide,

            // Function keys
            Keys.F1, Keys.F2, Keys.F3, Keys.F4, Keys.F5, Keys.F6,
            Keys.F7, Keys.F8, Keys.F9, Keys.F10, Keys.F11, Keys.F12,
            Keys.F13, Keys.F14, Keys.F15, Keys.F16, Keys.F17, Keys.F18,
            Keys.F19, Keys.F20, Keys.F21, Keys.F22, Keys.F23, Keys.F24,

            // Navigation & editing
            Keys.Back, Keys.Tab, Keys.Enter, Keys.Escape, Keys.Space,
            Keys.PageUp, Keys.PageDown, Keys.End, Keys.Home,
            Keys.Left, Keys.Up, Keys.Right, Keys.Down,
            Keys.Insert, Keys.Delete,

            // Special keys
            Keys.PrintScreen, Keys.Scroll, Keys.Pause,

            // OEM / punctuation
            //Keys.Oem1, Keys.Oem2, Keys.Oem3, Keys.Oem4, Keys.Oem5,
            //Keys.Oem6, Keys.Oem7, Keys.Oem8, Keys.Oem102
        };
}
