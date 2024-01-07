﻿global using Exceptionless;
global using Fabrela.Domain.Core.Interfaces.Entities;
global using Fabrela.Domain.Core.Interfaces.UnitOfWork;
global using FluentValidation;
global using MediatR;
global using Microsoft.AspNetCore.Http;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Storage;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;
global using NewShoreAir.Business.Domain.Entidades;
global using NewShoreAir.DataAccess.Persistencia;
global using NewShoreAir.Shared.Exceptions;
global using NewShoreAir.Shared.Models;
global using NewShoreAir.Shared.Services.Interfaces;
global using Newtonsoft.Json;
global using System.Data.Common;
global using System.Linq.Expressions;
global using System.Reflection;
