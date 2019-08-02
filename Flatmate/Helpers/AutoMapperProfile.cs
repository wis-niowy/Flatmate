using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flatmate.Models.EntityModels;
using Flatmate.ViewModels.Dashboard;
using AutoMapper;
using Flatmate.ViewModels.ExpenseManager;

namespace Flatmate.Helpers
{
    public class AutoMapperProfile : Profile
    {
        //public AutoMapperProfile()
        //{
        //    CreateMap<Expense, LiabilityExpenseViewModel>();
        //    CreateMap<ExpenseDebitor, CredibilityExpenseViewModel>()
        //        .ForMember(dest => dest.InitiatorFirstName,
        //            m => m.MapFrom(src => src.Expense.Initiator.FirstName))
        //        .ForMember(dest => dest.InitiatorLastName,
        //            m => m.MapFrom(src => src.Expense.Initiator.LastName))
        //        .ForMember(dest => dest.Value,
        //            m => m.MapFrom(src => src.Expense.Value))
        //        .ForMember(dest => dest.Date,
        //            m => m.MapFrom(src => src.Expense.Date))
        //        .ForMember(dest => dest.ExpenseSubject,
        //            m => m.MapFrom(src => src.Expense.ExpenseSubject))
        //        .ForMember(dest => dest.InitiatorId,
        //            m => m.MapFrom(src => src.Expense.InitiatorId))
        //        .ForMember(dest => dest.DebitorId,
        //            m => m.MapFrom(src => src.Debitor.UserId))
        //        .ForMember(dest => dest.DebitorFirstName,
        //            m => m.MapFrom(src => src.Debitor.FirstName))
        //        .ForMember(dest => dest.DebitorLastName,
        //            m => m.MapFrom(src => src.Debitor.LastName));
        //    CreateMap<int, ExpenseDebitor>()
        //        .ForMember(dest => dest.DebitorId,
        //            m => m.MapFrom(src => src));
        //    CreateMap<NewSingleExpenseViewModel, Expense>()
        //        .ForMember(dest => dest.DebitorsCollection,
        //            m => m.MapFrom(src => src.DebitorsCollection));
        //    CreateMap<ExpenseDebitor, int>()
        //        .ConvertUsing(source => source.DebitorId);
        //    CreateMap<Expense, EditOneTimeExpenseViewModel>()
        //        .ForMember(dest => dest.DebitorsCollection,
        //            m => m.MapFrom(src => src.DebitorsCollection));
        //    CreateMap<EditOneTimeExpenseViewModel, Expense>()
        //        .ForMember(dest => dest.DebitorsCollection,
        //            m => m.MapFrom(src => src.DebitorsCollection));
        //    //CreateMap<ExpenseListItemViewModel, Expense>()
        //    //    .ForMember(dest => dest.DebitorsCollection,
        //    //        m => m.MapFrom(src => src.DebitorsCollection));
        //    CreateMap<int, RecurringBillDebitor>()
        //        .ForMember(dest => dest.DebitorId,
        //            m => m.MapFrom(src => src));
        //    CreateMap<NewRecurringExpenseViewModel, RecurringBill>()
        //        .ForMember(dest => dest.DebitorsCollection,
        //            m => m.MapFrom(src => src.DebitorsCollection));
        //    CreateMap<int, OrderDebitor>()
        //        .ForMember(dest => dest.DebitorId,
        //            m => m.MapFrom(src => src));
        //    CreateMap<NewOrderViewModel, Order>()
        //        .ForMember(dest => dest.DebitorsCollection,
        //            m => m.MapFrom(src => src.DebitorsCollection));
        //    CreateMap<Expense, ExistingSingleExpenseViewModel>();
        //}
    }
}