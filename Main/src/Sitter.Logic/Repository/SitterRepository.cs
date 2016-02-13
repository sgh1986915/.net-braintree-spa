using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySitterHub.Model;
using MySitterHub.DAL.DataAccess;
using MySitterHub.Model.Core;

namespace MySitterHub.Logic.Repository
{
    public class SitterRepository
    {
        private readonly SitterDal _sitterDal = new SitterDal();
        private readonly AppUserDal _appUserDal = new AppUserDal();
        private readonly ParentDal _parentDal = new ParentDal();
        private readonly JobRepository _jobRepo = new JobRepository();

        public Sitter GetById(int id)
        {
            return _sitterDal.GetById(id);
        }

        public List<Sitter> GetAll()
        {
            return _sitterDal.GetAll();
        }

        public void DeleteById(int id)
        {
            _sitterDal.DeleteById(id);
        }

        public SitterMyClientsSM GetSitterMyClients(int sitterId)
        {
            var vm = new SitterMyClientsSM();
            Sitter sitter = GetById(sitterId);
            if (sitter == null)
                return null;

            vm.MyClients = new List<SitterMyClientSM>();

            var parentsWithSitter = _parentDal.GetAllParentIdsWhoHaveSitter(sitterId);

            foreach (int parentId in parentsWithSitter)
            {
                AppUser appUser = _appUserDal.GetById(parentId);

                var myClient = new SitterMyClientSM()
                {
                    ParentId = parentId,
                    FullName =  appUser.FullName(),
                    MobilePhone = appUser.MobilePhone
                };

                vm.MyClients.Add(myClient);
            }

            vm.MyClients = vm.MyClients.OrderBy(m => m.FullName).ToList();

            return vm;

        }

    }

    public class SitterMyClientsSM
    {
        public List<SitterMyClientSM> MyClients { get; set; }
 
    }

    public class SitterMyClientSM
    {
        public int ParentId { get; set; }
        public string FullName { get; set; }
        public string MobilePhone { get; set; }
    }

    public class SitterJobInviteResponseSM
    {        
        public int JobId { get; set; }
        public int SitterId { get; set; }
        public SitterResponse Response { get; set; }
        public string Message { get; set; }
    }

    public class SitterCancelAcceptedJobSM
    {
        public int JobId { get; set; }
        public int SitterId { get; set; }
    }

    public enum SitterResponse
    {
        Unrecognized,
        Accept,
        Decline,
        CancelAccept
    }

    public class SitterRequestPaymentSM
    {
        public int JobId { get; set; }
        public int SitterId { get; set; }
    }

}
