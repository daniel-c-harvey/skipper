﻿using System.ComponentModel.DataAnnotations;

namespace SkipperWeb.Shared.Email.Mailtrap.Models;

public class Address
{
    /// <summary>
    /// Email address.
    /// </summary>
    [Required]
    public string Email { get; set; }

    /// <summary>
    /// Name associated with the email address.
    /// </summary>
    public string Name { get; set; }
}
