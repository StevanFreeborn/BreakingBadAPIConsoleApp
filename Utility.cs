using System;
using Onspring.API.SDK;
using Onspring.API.SDK.Helpers;
using Onspring.API.SDK.Enums;
using Onspring.API.SDK.Models;
using System.Linq;

public static class Utility
{
    public static string GetResultValueString(RecordFieldValue value)
    {
        switch (value.Type)
        {
            case ResultValueType.String:
                return value.AsString();
            case ResultValueType.Integer:
                return $"{value.AsNullableInteger()}";
            case ResultValueType.Decimal:
                return $"{value.AsNullableDecimal()}";
            case ResultValueType.Date:
                return $"{value.AsNullableDateTime()}";
            case ResultValueType.TimeSpan:
                var data = value.AsTimeSpanData();
                return $"Quantity: {data.Quantity}, Increment: {data.Increment}, Recurrence: {data.Recurrence}, EndByDate: {data.EndByDate}, EndAfterOccurrences: {data.EndAfterOccurrences}";
            case ResultValueType.Guid:
                return $"{value.AsNullableGuid()}";
            case ResultValueType.StringList:
                return string.Join(", ", value.AsStringList());
            case ResultValueType.IntegerList:
                return string.Join(", ", value.AsIntegerList());
            case ResultValueType.GuidList:
                return string.Join(", ", value.AsGuidList());
            case ResultValueType.AttachmentList:
                var attachmentFiles = value.AsAttachmentList().Select(f => $"FileId: {f.FileId}, FileName: {f.FileName}, Notes: {f.Notes}");
                return string.Join(", ", attachmentFiles);
            case ResultValueType.ScoringGroupList:
                var scoringGroups = value.AsScoringGroupList().Select(g => $"ListValueId: {g.ListValueId}, Name: {g.Name}, Score: {g.Score}, MaximumScore: {g.MaximumScore}");
                return string.Join(", ", scoringGroups);
            case ResultValueType.FileList:
                return String.Join(", ", value.AsFileList());
            default:
                // e.g., future types not supported in this version
                return $"Unsupported ResultValueType: {value.Type}";
        }
    }
}
