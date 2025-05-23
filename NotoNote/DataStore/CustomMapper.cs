using LiteDB;
using NotoNote.Models;

namespace NotoNote.DataStore;
public class CustomMapper
{

    public CustomMapper()
    {
        BsonMapper.Global.RegisterType<ITranscriptionModel>(
            serialize:
            (id) =>
            {
                var doc = new BsonDocument();
                if (id is OpenAiCompatibleChatModel om)
                {
                    doc["Type"] = "OpenAiCompatibleChatModelId";
                    doc["DisplayName"] = om.DisplayName.Value;
                    doc["ApiId"] = om.ApiId.Value;
                    doc["ApiSource"] = om.ApiSource.ToString();
                }
                else
                {
                    throw new NotImplementedException($"Transcription model {id.GetType()} is not implemented.");
                }
                return doc;
            },
            deserialize:
            (doc) =>
            {
                var type = doc["Type"].AsString;
                if (type == "OpenAiCompatibleChatModelId")
                {
                    return new OpenAiCompatibleTranscriptionModel(
                        new ModelName(doc["DisplayName"].AsString),
                        new ApiModelId(doc["ApiId"].AsString),
                        (ApiSource)Enum.Parse(typeof(ApiSource), doc["ApiSource"].AsString));
                }

                throw new NotImplementedException($"Transcription model {type} is not implemented.");
            });

        BsonMapper.Global.RegisterType<IChatModel>(
            serialize:
            (id) =>
            {
                var doc = new BsonDocument();
                if (id is OpenAiCompatibleChatModel om)
                {
                    doc["Type"] = "OpenAiCompatibleChatModelId";
                    doc["DisplayName"] = om.DisplayName.Value;
                    doc["ApiId"] = om.ApiId.Value;
                    doc["ApiSource"] = om.ApiSource.ToString();
                }
                else
                {
                    throw new NotImplementedException($"Transcription model {id.GetType()} is not implemented.");
                }
                return doc;
            },
            deserialize:
            (doc) =>
            {
                var type = doc["Type"].AsString;
                if (type == "OpenAiCompatibleChatModelId")
                {
                    return new OpenAiCompatibleChatModel(
                        new ModelName(doc["DisplayName"].AsString),
                        new ApiModelId(doc["ApiId"].AsString),
                        (ApiSource)Enum.Parse(typeof(ApiSource), doc["ApiSource"].AsString));
                }
                throw new NotImplementedException($"Transcription model {type} is not implemented.");
            });
    }
}

