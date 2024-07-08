global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.Extensions.Options;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.OpenApi.Models;
global using System.ComponentModel.DataAnnotations;
global using System.Security.Claims;
global using System.Net;
global using System.Text.Json;
global using Swashbuckle.AspNetCore.SwaggerGen;

global using OneOf;
global using OneOf.Types;
global using FluentValidation;
global using FluentValidation.Results;
global using MediatR;
global using AutoMapper;
global using HotChocolate.Authorization;

global using DevBook.API;
global using DevBook.API.Exceptions;
global using DevBook.API.Infrastructure;
global using DevBook.API.Middleware;
global using DevBook.API.Identity;
global using DevBook.API.Infrastructure.Commands;
global using DevBook.API.Infrastructure.Queries;
global using DevBook.API.Infrastructure.Validation;
