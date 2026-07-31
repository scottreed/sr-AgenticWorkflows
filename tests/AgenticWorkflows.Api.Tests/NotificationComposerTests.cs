using AgenticWorkflows.Api.Models;
using AgenticWorkflows.Api.Services;

namespace AgenticWorkflows.Api.Tests;

public sealed class NotificationComposerTests
{
    private static WorkItem MakeItem(string? description = null, DateOnly? dueDate = null) =>
        new(Guid.NewGuid(), "Test Item", description, 3, WorkItemStatus.Todo, dueDate);

    [Fact]
    public void BuildCreatedNotification_truncates_description_longer_than_90_characters()
    {
        var longDescription = new string('a', 91);
        var item = MakeItem(description: longDescription);

        var notification = NotificationComposer.BuildCreatedNotification(item);

        Assert.Contains("Description: " + new string('a', 87) + "...", notification);
    }

    [Fact]
    public void BuildDueSoonNotification_truncates_description_longer_than_90_characters()
    {
        var longDescription = new string('b', 95);
        var item = MakeItem(description: longDescription);

        var notification = NotificationComposer.BuildDueSoonNotification(item);

        Assert.Contains("Description: " + new string('b', 87) + "...", notification);
    }

    [Fact]
    public void BuildCreatedNotification_omits_due_date_line_when_DueDate_is_null()
    {
        var item = MakeItem(dueDate: null);

        var notification = NotificationComposer.BuildCreatedNotification(item);

        Assert.DoesNotContain("Due date:", notification);
    }

    [Fact]
    public void BuildDueSoonNotification_omits_due_date_line_when_DueDate_is_null()
    {
        var item = MakeItem(dueDate: null);

        var notification = NotificationComposer.BuildDueSoonNotification(item);

        Assert.DoesNotContain("Due date:", notification);
    }

    [Fact]
    public void BuildCreatedNotification_contains_expected_next_step_text()
    {
        var item = MakeItem();

        var notification = NotificationComposer.BuildCreatedNotification(item);

        Assert.Contains("Next step: Review the backlog and assign an owner.", notification);
    }

    [Fact]
    public void BuildDueSoonNotification_contains_expected_next_step_text()
    {
        var item = MakeItem();

        var notification = NotificationComposer.BuildDueSoonNotification(item);

        Assert.Contains("Next step: Confirm the item still belongs in this sprint.", notification);
    }
}
