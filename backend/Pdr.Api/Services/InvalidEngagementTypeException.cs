namespace Pdr.Api.Services;

public class InvalidEngagementTypeException(int engagementTypeId)
    : Exception($"Engagement type with id {engagementTypeId} does not exist.");
