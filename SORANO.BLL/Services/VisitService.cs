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
            var visits = UnitOfWork.Get<Visit>()
                .GetAll()
                .OrderByDescending(a => a.Date)
                .Select(v => new VisitDto
                {
                    ID = v.ID,
                    Code = v.Code.ToUpper(),
                    Date = v.Date,
                    LocationID = v.LocationID,
                    LocationName = v.Location.Name
                })
                .ToList();

            return new SuccessResponse<IEnumerable<VisitDto>>(visits);
        }

        public async Task<ServiceResponse<VisitDto>> GetAsync(int id)
        {
            var visit = await UnitOfWork.Get<Visit>()
                .GetAsync(v => v.ID == id, v => v.Location);

            return visit == null
                ? new ServiceResponse<VisitDto>(ServiceResponseStatus.NotFound)
                : new SuccessResponse<VisitDto>(new VisitDto
                {
                    ID = visit.ID,
                    Code = visit.Code.ToUpper(),
                    Date = visit.Date,
                    LocationID = visit.LocationID,
                    LocationName = visit.Location.Name
                });
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

        public async Task<ServiceResponse<VisitDto>> UpdateAsync(VisitDto model, int userId)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var visit = UnitOfWork.Get<Visit>().Get(model.ID, v => v.Visitors);
            visit.LocationID = model.LocationID;
            visit.Code = model.Code;
            visit.Date = model.Date;
            visit.UpdateModifiedFields(userId);
            visit.Visitors.ToList().ForEach(v => UnitOfWork.Get<Visitor>().Delete(v));
            visit.Visitors.Clear();

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

            var updated = UnitOfWork.Get<Visit>().Update(visit);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<VisitDto>(new VisitDto
            {
                ID = updated.ID,
                Code = updated.Code.ToUpper(),
                Date = updated.Date,
                LocationID = updated.LocationID,
                LocationName = updated.Location?.Name
            });
        }

        public async Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            var existentVisit = await UnitOfWork.Get<Visit>().GetAsync(v => v.ID == id, v => v.Visitors);

            if (existentVisit == null)
                return new ServiceResponse<int>(ServiceResponseStatus.NotFound);

            existentVisit.Visitors.ToList().ForEach(v =>
            {
                UnitOfWork.Get<Visitor>().Delete(v);
            });

            UnitOfWork.Get<Visit>().Delete(existentVisit);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(id);
        }        
    }
}