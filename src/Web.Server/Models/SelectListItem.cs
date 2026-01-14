using System;

namespace Web.Server.Models;

public record SelectListItem(string Value, string Text, bool Disabled = false);