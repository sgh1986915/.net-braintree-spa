using System;
using System.Collections.Generic;
using System.Linq;
using MySitterHub.DAL.DataAccess;
using MySitterHub.Logic.Messaging;
using MySitterHub.Model.Core;
using MySitterHub.Model.Misc;

namespace MySitterHub.Logic.Repository
{
    public class ParentRepository
    {
        public const string ActionSuccess = "good";
        private readonly AppUserDal _appUserDal = new AppUserDal();
        private readonly JobDal _jobDal = new JobDal();
        private readonly ParentDal _parentDal = new ParentDal();
        private readonly SitterDal _sitterDal = new SitterDal();
        private readonly UserPassDal _userPassDal = new UserPassDal();

        public List<Parent> GetAll()
        {
            return _parentDal.GetAll();
        }

        public Parent GetById(int id, bool includeUser = true)
        {
            Parent p = _parentDal.GetById(id);
            if (includeUser)
            {
                p.User = _appUserDal.GetById(id);
            }
            return p;
        }

        public ParentMySittersDataSM GetSittersForParent(int parentId)
        {
            var vm = new ParentMySittersDataSM();
            Parent parent = GetById(parentId, false);
            if (parent == null)
                return null;

            vm.Sitters = new List<ParentMySitterSM>();
            if (parent.Sitters != null)
            {
                foreach (ParentMySitter pms in parent.Sitters)
                {
                    Sitter s = _sitterDal.GetById(pms.SitterId);

                    var pmsv = new ParentMySitterSM
                    {
                        Id = s.Id,
                        Age = s.Age,
                        Email = s.User.Email,
                        FirstName = s.User.FirstName,
                        LastName = s.User.LastName,
                        MobilePhone = s.User.MobilePhone,
                        Rate = pms.Rate,
                        SortOrder = pms.SortOrder
                    };

                    vm.Sitters.Add(pmsv);
                }
            }
            vm.Sitters = vm.Sitters.OrderBy(m => m.SortOrder).ToList();

            vm.SitterInvites = new List<ParentMySitterInviteVM>();
            if (parent.InviteToSignup != null)
            {
                foreach (InviteToSignup psi in parent.InviteToSignup)
                {
                    var psiv = new ParentMySitterInviteVM
                    {
                        InviteStatus = psi.InviteStatus,
                        MobilePhoneInvite = psi.MobilePhone,
                        InviteNickName =  psi.InviteNickName
                    };
                    vm.SitterInvites.Add(psiv);
                }
            }

            return vm;
        }

        public int GetSitterCountForParent(int parentId)
        {
            Parent parent = GetById(parentId, false);
            return parent.Sitters == null ? 0 : parent.Sitters.Count;
        }

        public Parent GetOneParentInviteSitterToSignup()
        {
            var p = _parentDal.GetOneParentInviteSitterToSignup();
            if (p == null)
                return null;

            p.User = _appUserDal.GetById(p.Id);
            return p;
        }
        /// <summary>
        ///     Does not update User property of parent (just list of sitters)
        /// </summary>
        public bool UpdateParent(Parent p)
        {
            _parentDal.Update(p);
            return true;
        }

        public void DeleteById(int id)
        {
            _parentDal.DeleteById(id);
        }

        public string SaveSittersForParent(int userId, List<ParentMySitterSaveSM> sfps)
        {
            string notice = ActionSuccess;
            Parent p = _parentDal.GetById(userId);
            if (p == null)
                throw new Exception("Parent not found with id " + userId);

            foreach (ParentMySitterSaveSM item in sfps)
            {
                ParentMySitter sitterMatch = p.Sitters.FirstOrDefault(m => m.SitterId == item.Id);
                if (sitterMatch == null)
                {
                    notice = string.Format("Sitter with id {0} not found for parent", item.Id);
                }
                else if (item.DeleteSitterOnSave)
                {
                    p.Sitters.Remove(sitterMatch);
                }
                else
                {
                    sitterMatch.Rate = item.Rate;
                    sitterMatch.SortOrder = item.SortOrder;
                }
            }
            _parentDal.Update(p);

            return notice;
        }

        public ServiceResult AddSitterInviteByMobile(int parentId, InviteToSignup sitterInvite)
        {
            var result = new ServiceResult();

            // STEP - Validate
            ValidateInvite(sitterInvite, result);
            if (result.Message != null)
                return result;

            // STEP - Populate rest of fields
            sitterInvite.MobilePhone = PhoneUtil.CleanAndEnsureCountryCode(sitterInvite.MobilePhone);
            sitterInvite.InviteDate = TimeUtil.GetCurrentUtcTime();
            sitterInvite.InviteStatus = InvitationState.InvitePending;

            // STEP - Get Parent
            Parent p = _parentDal.GetById(parentId);
            if (p == null)
                throw new AppException("Parent not found with ID " + parentId);
            if (p.InviteToSignup == null)
                p.InviteToSignup = new List<InviteToSignup>();
            if (p.Sitters == null)
                p.Sitters = new List<ParentMySitter>();


            // STEP - Check if Invited mobile is already in Parent - MySitters.
            AppUser usr = _appUserDal.GetByMobile(sitterInvite.MobilePhone);
            if (usr != null) // Invited Mobile does not belong to an existing user.
            {
                if (usr.UserRole != UserRole.Sitter)
                {
                    result.Message = "Invited User is parent which is not supported at this time.";
                    return result;
                }

                // STEP - Check if sitter already in parent mysitters
                foreach (ParentMySitter sid in p.Sitters)
                {
                    Sitter sf = _sitterDal.GetById(sid.SitterId);
                    if (sf != null && sf.User.MobilePhone == sitterInvite.MobilePhone)
                    {
                        result.Message = "Parent already has sitter in network with mobile " + sitterInvite.MobilePhone;
                        return result;
                    }
                }
            }

            if (p.InviteToSignup.Any(m => m.MobilePhone == sitterInvite.MobilePhone))
            {
                result.Message = string.Format("Mobile '{0}' has already been invited.", sitterInvite.MobilePhone);
                return result;
            }
            p.InviteToSignup.Add(sitterInvite);

            // STEP - Persist
            _parentDal.Update(p);

            // STEP - Mark as successfully added
            result.IsSuccess = true;
            return result;
        }

        private void ValidateInvite(InviteToSignup sitterInvite, ServiceResult result)
        {
            if (string.IsNullOrWhiteSpace(sitterInvite.MobilePhone))
            {
                result.Message = "Mobile phone is required";
            }
            else if (!PhoneUtil.IsValidPhoneNumber(sitterInvite.MobilePhone))
            {
                result.Message = "Mobile phone is not a valid phone";
            }
        }

        public ServiceResult CancelInviteSitter(int parentId, string sitterMobilePhone, bool isSitterDecline = false)
        {
            var result = new ServiceResult();

            if (string.IsNullOrWhiteSpace(sitterMobilePhone))
            {
                result.Message = "Mobile phone is required";
                return result;
            }
            else if (!PhoneUtil.IsValidPhoneNumber(sitterMobilePhone))
            {
                result.Message = "Mobile phone is not a valid phone";
                return result;
            }
            sitterMobilePhone = PhoneUtil.CleanAndEnsureCountryCode(sitterMobilePhone);

            var p = _parentDal.GetById(parentId);
            if (p == null)
            {
                result.Message = "Parent not found";
                return result;
            }
            var invite = p.InviteToSignup.FirstOrDefault(m => m.MobilePhone == sitterMobilePhone);
            if (invite == null)
            {
                result.Message = "Unable to cancel invite, mobile phone not found.";
                return result;
            }
            if (isSitterDecline)
            {
                invite.InviteStatus = InvitationState.Declined;
            }
            else
            {
                p.InviteToSignup.Remove(invite);                
            }
            _parentDal.Update(p);

            result.IsSuccess = true;
            return result;
        }

        public bool AddSitter(int id,ParentMySitter pms)
        {
            var p = GetById(id,false);
            p.Sitters.Add(pms);
            _parentDal.Update(p);
            return true;
        }
    }

    public class NewMySitterInviteVM
    {
        public string MobilePhone { get; set; }
    }

    public class ServiceResult
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class ParentMySitterInviteVM
    {
        public string MobilePhoneInvite { get; set; }
        public string InviteNickName { get; set; }
        public DateTime InviteDate { get; set; }
        public InvitationState InviteStatus { get; set; }
        public string SitterInviteResponse { get; set; }
    }

    public class ParentMySitterSaveSM : ParentMySitterSM
    {
        public bool DeleteSitterOnSave { get; set; }
    }

    public class ParentMySitterSM
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string PhotoUrl { get; set; }
        public decimal Rate { get; set; }
        public int SortOrder { get; set; }
    }

    public class ParentMySittersDataSM
    {
        public List<ParentMySitterSM> Sitters { get; set; }
        public List<ParentMySitterInviteVM> SitterInvites { get; set; }
    }
}