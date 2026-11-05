// EduSubmit – lát cắt 1: Submission.Client (vai trò MÁY KHÁCH)
// Sinh viên: Nguyễn Thị Quỳnh Thơ - MSSV: dtc245340007

using System.Net.Http.Json;

// 1. Địa chỉ API lấy từ tham số dòng lệnh hoặc mặc định localhost:5000
var apiBaseUrl = args.Length > 0 ? args[0] : "http://localhost:5000";

using var http = new HttpClient {
    BaseAddress = new Uri(apiBaseUrl)
};

// 2. Gọi GET /api/assignments
Console.WriteLine("=== LAY DANH SACH BAI TAP ===");
var assignments = await http.GetFromJsonAsync<List<Assignment>>("/api/assignments");
Console.WriteLine("Danh sach bai tap:");
foreach (var assignment in assignments ?? new List<Assignment>()) {
    Console.WriteLine($"  - {assignment.Id}: {assignment.Title} ({assignment.Description})");
}

// 3. Gọi POST /api/submissions
Console.WriteLine("\n=== GUI BAI NOP MOI ===");
var request = new CreateSubmissionRequest(
    AssignmentId: 1,
    StudentCode: "dtc245340007",
    RepositoryUrl: "https://github.com/nguyenthiquynhtho/EduSubmit-dtc245340007");

var response = await http.PostAsJsonAsync("/api/submissions", request);
response.EnsureSuccessStatusCode();

var result = await response.Content.ReadFromJsonAsync<SubmissionResult>();
Console.WriteLine($"Da nop bai thanh cong!");
Console.WriteLine($"  Ma luot nop: {result?.Id}");
Console.WriteLine($"  Trang thai : {result?.Status}");
Console.WriteLine($"  Thoi gian  : {result?.ReceivedAt}");

// 4. Bước 12 (Bài mở rộng): Gọi GET /api/submissions/{id} để đọc lại bài vừa nộp
if (result != null) {
    Console.WriteLine($"\n=== [BAI MO RONG] DOC LAI LUOT NOP THEO ID ===");
    var getResponse = await http.GetAsync($"/api/submissions/{result.Id}");
    getResponse.EnsureSuccessStatusCode();

    var fetchedSubmission = await getResponse.Content.ReadFromJsonAsync<SubmissionResult>();
    Console.WriteLine($"Doc thanh cong luot nop tu API:");
    Console.WriteLine($"  ID         : {fetchedSubmission?.Id}");
    Console.WriteLine($"  Sinh vien  : {fetchedSubmission?.StudentCode}");
    Console.WriteLine($"  Bai tap    : {fetchedSubmission?.AssignmentId}");
    Console.WriteLine($"  Trang thai : {fetchedSubmission?.Status}");
}

// Khai báo kiểu dữ liệu trao đổi
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
