using Microsoft.AspNetCore.Mvc;
using webapi.Data;
using webapi.Services;

namespace webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class TimeSheetController : ControllerBase
{
    private readonly DataAccessor _dataAccessor;
    private readonly ILogger<TimeSheetController> _logger;

    public TimeSheetController(DataAccessor dataAccessor, ILogger<TimeSheetController> logger)
    {
        _dataAccessor = dataAccessor;
        _logger = logger;
    }

    [HttpGet]
    [Route("/GetRecords")]
    public async Task<DataOperationResult> GetRecords()
    {
        var response = await _dataAccessor.GetRecordsAsync();
        SetResponseMessage(response,
            $"Записи из таблицы получены",
            "Не удалось получить записи из базы данных. Проверьте логи сервера.");

        return response;
    }

    [HttpGet]
    [Route("/CreateTable")]
    public async Task<DataOperationResult> CreateTable()
    {
        var result = await _dataAccessor.CreateTableAsync();
        var response = new DataOperationResult(result);
        SetResponseMessage(response,
            "Таблица в базе данных создана",
            "Невозможно создать таблицу в базе данных. Проверьте логи сервера.");

        return response;
    }

    [HttpPost]
    [Route("/AddRecord")]
    public async Task<DataOperationResult> AddRecord([FromBody] Record record)
    {
        var result = await _dataAccessor.AddRecordAsync(record);
        var response = new DataOperationResult(result);

        SetResponseMessage(response,
            "Новая запись добавлена",
            "Не удалось добавить новую запись. Проверьте логи сервера.");

        return response;
    }

    [HttpGet]
    [Route("/DeleteRecord")]
    public async Task<DataOperationResult> DeleteRecord(int id)
    {
        var result = await _dataAccessor.DeleteRecordByIdAsync(id);
        var response = new DataOperationResult(result);

        SetResponseMessage(response,
            $"Запись с идентификатором {id} удалена",
            $"Не удалось удалить запись с идентификатором {id}. Проверьте логи сервера.");

        return response;
    }

    [HttpPost]
    [Route("/EditRecord")]
    public async Task<DataOperationResult> EditRecord([FromBody] Record record)
    {
        var result = await _dataAccessor.EditRecordAsync(record);
        var response = new DataOperationResult(result);

        if (result) response.Message = $"Запись с идентификатором {record.id} обновлена";
        else response.Message = $"Не удалось обновить запись с идентификатором {record.id}";

        return response;
    }

    private void SetResponseMessage(DataOperationResult response, string success, string failed)
    {
        if (response.IsSuccess) response.Message = success;
        else response.Message = failed;
    }
}
