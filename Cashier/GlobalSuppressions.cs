﻿// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "Not needed for asp.net projects.")]
[assembly: SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Application is not localised.")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "This is the specific exception used", Scope = "member", Target = "~M:Cashier.Engine.CoffeeMachineService.SendRequestAsync(System.String,System.Uri)~System.Threading.Tasks.Task{System.Boolean}")]
