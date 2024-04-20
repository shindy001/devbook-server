using AutoMapper;

namespace DevBook.API.Mapping;

internal interface IMappebleTo<T>
{
	virtual void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType()).ReverseMap();
}
