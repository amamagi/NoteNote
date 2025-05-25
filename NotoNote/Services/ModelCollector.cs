using Microsoft.Extensions.Options;
using NotoNote.Models;

namespace NotoNote.Services;
public class ModelCollector : ITranscriptionModelProvider, IChatModelProvider, IApiMetadataProvider
{
    private readonly Dictionary<ApiSource, ApiMetadata> _apiMetadates = [];
    private readonly List<ITranscriptionModel> _transcriptionModels = [];
    private readonly List<IChatModel> _chatModels = [];

    public ModelCollector(IOptions<List<OpenAiCompatibleApiOptions>> options)
    {
        // Collect OpenAI Compatible APIs
        foreach (var option in options.Value)
        {
            // Collect API Metadata
            ApiSource source;
            try
            {
                source = new ApiSource(new(option.Name));
                // 未定義のAPIソースについてはAPIキーはOptionとする
                bool requireApiKey = source == ApiSource.OpenAI || source == ApiSource.Gemini || source == ApiSource.Anthropic;
                var apiMetadata = new ApiMetadata(source, new(option.BaseUrl), requireApiKey);
                _apiMetadates.Add(source, apiMetadata);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"API Metadata Error: {ex.Message}");
                continue;
            }

            // Collect Transcription models
            foreach (var model in option.TranscriptionModels)
            {
                try
                {
                    var id = new ApiModelId(model.ApiId);
                    var name = new ModelName(model.Name);
                    var apiId = new ApiModelId(model.ApiId);
                    var transcriptionModel = new OpenAiCompatibleTranscriptionModel(name, apiId, source);
                    _transcriptionModels.Add(transcriptionModel);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Transcription Model Error: {ex.Message}");
                }
            }

            // Collect Chat models
            foreach (var model in option.ChatModels)
            {
                try
                {
                    var id = new ApiModelId(model.ApiId);
                    var name = new ModelName(model.Name);
                    var apiId = new ApiModelId(model.ApiId);
                    var chatModel = new OpenAiCompatibleChatModel(name, apiId, source);
                    _chatModels.Add(chatModel);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Chat Model Error: {ex.Message}");
                    continue;
                }
            }
        }
    }

    public ApiMetadata Get(ApiSource source) => _apiMetadates[source];

    IEnumerable<ITranscriptionModel> ITranscriptionModelProvider.GetAll() => _transcriptionModels;
    IEnumerable<IChatModel> IChatModelProvider.GetAll() => _chatModels;
}
