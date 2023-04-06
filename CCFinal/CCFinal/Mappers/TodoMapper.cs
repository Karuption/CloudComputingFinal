using CCFinal.Dtos;
using CCFinal.Entities;
using Riok.Mapperly.Abstractions;

namespace CCFinal.Mappers;

[Mapper(EnumMappingStrategy = EnumMappingStrategy.ByName)]
public partial class TodoMapper:ITodoMapper {
    public partial ToDoTask ToDoTaskDtoToModel(ToDoTaskDTO task);
    public partial ToDoTaskDTO ToDoTaskToDto(ToDoTask dto);
}
public interface ITodoMapper {
    public ToDoTask ToDoTaskDtoToModel(ToDoTaskDTO task);
    public ToDoTaskDTO ToDoTaskToDto(ToDoTask dto);
}
