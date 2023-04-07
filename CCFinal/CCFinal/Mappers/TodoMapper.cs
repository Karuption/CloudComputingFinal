using CCFinal.Dtos;
using CCFinal.Entities;
using Riok.Mapperly.Abstractions;

namespace CCFinal.Mappers;

[Mapper(EnumMappingStrategy = EnumMappingStrategy.ByName)]
public partial class TodoMapper:ITodoMapper {
    public partial ToDoTask TodoTaskDtoToModel(ToDoTaskDTO task);
    public partial ToDoTaskDTO TodoTaskToDto(ToDoTask dto);
    public partial ToDoTask TodoTaskDtoToModel(ToDoTaskIntegrationDto task);
    public partial ToDoTaskIntegrationDto TodoTaskModelToDto(ToDoTask task);
}
public interface ITodoMapper {
    public ToDoTask TodoTaskDtoToModel(ToDoTaskDTO task);
    public ToDoTaskDTO TodoTaskToDto(ToDoTask dto);
    public ToDoTask TodoTaskDtoToModel(ToDoTaskIntegrationDto task);
    public ToDoTaskIntegrationDto TodoTaskModelToDto(ToDoTask task);
}
