﻿
namespace Models.Shared.InputModels;

public interface IInputModel

{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}