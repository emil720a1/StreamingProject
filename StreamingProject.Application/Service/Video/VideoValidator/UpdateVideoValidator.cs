using FluentValidation;
using StreamingProject.Contracts.VideoDto.Crud;

namespace StreamingProject.Application.Service.Video.VideoValidator;

public class UpdateVideoValidator : AbstractValidator<UpdateVideoDto>
{
 public UpdateVideoValidator()
 {
     RuleFor(x => x.userId)
         .NotEmpty().WithMessage("UserId is required");
       
       
   RuleFor(x => x.Id)   
       .NotEmpty().WithMessage("Id is required");
   
   RuleFor(x => x.Title)
       .NotEmpty().WithMessage("Title is required");
 }   
}