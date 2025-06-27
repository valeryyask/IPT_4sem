using Microsoft.AspNetCore.HttpLogging; // ������ ������������ ���� ��� ����������� HTTP-�������� � �������

internal class Program // ���������� ����������� ������ Program
{
    private static void Main(string[] args) // ����� ����� � ���������
    {
        var builder = WebApplication.CreateBuilder(args); // �������� ������� ��� ��������� � ������� ���-����������

        // ��������� �������� ��� ����������� HTTP-�������� � �������
        builder.Services.AddHttpLogging(o =>
        {
            o.LoggingFields = HttpLoggingFields.RequestMethod | // ����������� ������ HTTP-������� (GET, POST � �.�.)
            HttpLoggingFields.RequestPath | // ����������� ���� �������
            HttpLoggingFields.ResponseStatusCode | // ����������� ������-���� ������
            HttpLoggingFields.ResponseBody; // ����������� ���� ������
        });

        // ��������� ���������� �����: ������ ���� ������ Information
        builder.Logging.AddFilter("Microsoft.AspNetCore.HttpLogging", LogLevel.Information);

        var app = builder.Build(); // ���������� ����������

        app.UseHttpLogging(); // ��������� middleware ��� ����������� HTTP-�������� � �������

        app.MapGet("/", () => "�� ������ ASPA"); // ��������� �������� ��� GET-�������� �� ��������� ���� "/"

        app.Run(); // ������ ����������
    }
}