// EduSubmit – lát cắt 1: Submission.Api (vai trò MÁY CHỦ)
// Sinh viên: Nguyễn Thị Quỳnh Thơ - MSSV: dtc245340007

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Dữ liệu danh sách bài tập mẫu
var assignments = new[]
{
    new Assignment(1, "Bai tap 1", "Tao API dau tien"),
    new Assignment(2, "Bai tap 2", "Goi API tu client")
};

// Bước 12 (Bài mở rộng): Danh sách lưu các bài nộp trong bộ nhớ
var submissions = new List<SubmissionResult>();

// GET / : kiểm tra dịch vụ đang chạy
app.MapGet("/", () => Results.Ok(new {
    service = "Submission.Api",
    status = "running"
}));

// GET /api/assignments : lấy danh sách bài tập
app.MapGet("/api/assignments", () => Results.Ok(assignments));

// POST /api/submissions : gửi một lượt nộp bài
app.MapPost("/api/submissions", (CreateSubmissionRequest request) => {
    // 1. Kiểm tra dữ liệu đầu vào
    if (request.AssignmentId <= 0 ||
        string.IsNullOrWhiteSpace(request.StudentCode) ||
        string.IsNullOrWhiteSpace(request.RepositoryUrl)) {
        return Results.BadRequest(new { message = "Du lieu nop bai khong hop le" });
    }

    // 2. Tạo lượt nộp mới với GUID ngẫu nhiên và trạng thái "received"
    var submission = new SubmissionResult(
        Guid.NewGuid(),
        request.AssignmentId,
        request.StudentCode,
        "received",
        DateTimeOffset.UtcNow);

    // Lưu vào danh sách (Bước 12)
    submissions.Add(submission);

    // 3. Trả 201 Created kèm header Location
    return Results.Created($"/api/submissions/{submission.Id}", submission);
});

// GET /api/submissions/{id:guid} : Bước 12 (Bài mở rộng) - đọc chi tiết lượt nộp
app.MapGet("/api/submissions/{id:guid}", (Guid id) => {
    var submission = submissions.FirstOrDefault(s => s.Id == id);
    if (submission is null) {
        return Results.NotFound(new { message = $"Khong tim thay luot nop voi id {id}" });
    }
    return Results.Ok(submission);
});

app.Run();

// Hợp đồng dữ liệu
record Assignment(int Id, string Title, string Description);

record CreateSubmissionRequest(
    int AssignmentId,
    string StudentCode,
    string RepositoryUrl);

record SubmissionResult(
    Guid Id,
    int AssignmentId,
    string StudentCode,
    string Status,
    DateTimeOffset ReceivedAt);
