namespace Pdr.Api.Exceptions;

public class StakeholderRoleAlreadyExistsException(string role)
    : AppException($"A communication cadence for stakeholder role '{role}' already exists for this engagement.", 400);
