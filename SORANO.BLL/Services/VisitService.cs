using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;
using SORANO.BLL.Extensions;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;

namespace SORANO.BLL.Services
{
    public class VisitService : BaseService, IVisitService
    {
        public VisitService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public ServiceResponse<IEnumerable<VisitDto>> GetAll(bool withDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<VisitDto>> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<int>> CreateAsync(VisitDto model, int userId)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var visit = model.ToEntity();

            visit
                .UpdateCreatedFields(userId)
                .UpdateModifiedFields(userId);                

            Regex.Matches(model.Code, @"^(([мМжЖ]+[1234]{1})+)$")
                .OfType<Match>()
                .ToList()
                .ForEach(visitorCode =>
                {
                    var code = visitorCode.Value;
                    var ageGroup = (VisitorAgeGroups)Enum.Parse(typeof(VisitorAgeGroups), code.Last().ToString());
                    code.Substring(0, code.Length - 1)
                        .ToLower()
                        .Select(c => c)
                        .ToList()
                        .ForEach(genderCode =>
                        {
                            var visitor = new Visitor
                            {
                                AgeGroup = ageGroup,
                                Gender = genderCode.Equals('м')
                                    ? VisitorGenders.Male
                                    : VisitorGenders.Female
                            };

                            visitor
                                .UpdateCreatedFields(userId)
                                .UpdateModifiedFields(userId);

                            visit.Visitors.Add(visitor);
                        });
                });

            var added = UnitOfWork.Get<Visit>().Add(visit);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(added.ID);           
        }

        public Task<ServiceResponse<VisitDto>> UpdateAsync(VisitDto model, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            throw new NotImplementedException();
        }        
    }
}