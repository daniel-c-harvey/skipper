﻿namespace Models.Shared.InputModels;

public class InputModelBase : IInputModel
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}