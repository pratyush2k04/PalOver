using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Common;
using Pdr.Api.DTOs.Notifications;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages the current user's notifications.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class NotificationsController(
    INotificationService notificationService,
    ICurrentUserService currentUserService) : ControllerBase
{
    /// <summary>
    /// Retrieves a page of the current user's notifications, optionally filtered by engagement.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<NotificationDto>>> GetNotifications(
        [FromQuery] int? engagementId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await notificationService.GetForUserAsync(currentUserService.GetUserId(), engagementId, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Returns the count of unread, active notifications for the current user.
    /// </summary>
    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        var count = await notificationService.GetUnreadCountAsync(currentUserService.GetUserId());
        return Ok(count);
    }

    /// <summary>
    /// Marks a single notification as read at the current server time.
    /// </summary>
    [HttpPut("{id:int}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await notificationService.MarkAsReadAsync(id, currentUserService.GetUserId());
        return NoContent();
    }

    /// <summary>
    /// Marks all of the current user's unread notifications as read.
    /// </summary>
    [HttpPut("read-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        await notificationService.MarkAllAsReadAsync(currentUserService.GetUserId());
        return NoContent();
    }

    /// <summary>
    /// Deletes a batch of notifications by id, regardless of who received them. Admin only.
    /// </summary>
    [HttpDelete]
    [Authorize(Policy = MasterChecklistConstants.Policies.AdminOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteNotificationsRequest request)
    {
        await notificationService.DeleteAsync(currentUserService.GetUserId(), request.Ids);
        return NoContent();
    }
}
