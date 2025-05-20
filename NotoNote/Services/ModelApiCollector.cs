using Microsoft.Extensions.Options;
using NotoNote.Models;

namespace NotoNote.Services;
public class ModelApiCollector : ITranscriptionModelProvider, IChatModelProvider
{
    private readonly Dictionary<TranscriptionModelId, ITranscriptionModel> _transcriptionModels = [];
    private readonly Dictionary<ChatModelId, IChatModel> _chatModels = [];

    public ModelApiCollector(IOptions<List<OpenAiCompatibleApiOptions>> options)
    {
        foreach (var option in options.Value)
        {
            var source = new ApiSourceWithUrl(new(option.Name), new(option.BaseUrl));
            var sourceId = option.Name.ToLower();

            foreach (var model in option.TranscriptionModels)
            {
                var id = new TranscriptionModelId($"{sourceId}-{model.ApiId}");
                var name = new ModelName(model.Name);
                var apiId = new ApiModelId(model.ApiId);
                var transcriptionModel = new OpenAiCompatibleTranscribeModel(id, name, source, apiId);
                if (_transcriptionModels.ContainsKey(id))
                {
                    MessageBox.Show($"Duplicate transcription model ID found: {id}. This will be ignored.");
                    continue;
                }
                _transcriptionModels[id] = transcriptionModel;
            }
            foreach (var model in option.ChatModels)
            {
                var id = new ChatModelId($"{sourceId}-{model.ApiId}");
                var name = new ModelName(model.Name);
                var apiId = new ApiModelId(model.ApiId);
                var chatModel = new OpenAiCompatibleChatModel(id, name, source, apiId);
                if (_chatModels.ContainsKey(id))
                {
                    MessageBox.Show($"Duplicate chat model ID found: {id}. This will be ignored.");
                    continue;
                }
                _chatModels[id] = chatModel;
            }
        }

    }

    public ITranscriptionModel? Get(TranscriptionModelId id)
    {
        if (_transcriptionModels.TryGetValue(id, out var model))
        {
            return model;
        }
        return null;
    }
    IEnumerable<ITranscriptionModel> ITranscriptionModelProvider.GetAll() => _transcriptionModels.Values;
    public IChatModel? Get(ChatModelId id)
    {
        if (_chatModels.TryGetValue(id, out var model))
        {
            return model;
        }
        return null;
    }
    IEnumerable<IChatModel> IChatModelProvider.GetAll() => _chatModels.Values;

}
