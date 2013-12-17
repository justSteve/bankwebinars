using System.Collections.Generic;
using System.Linq;

namespace CUWebinars.Web.Core
{
    public abstract class AbstractDTOAssembler<TDTO, TEntity> : IDTOAssembler<TDTO, TEntity>
    {
        public abstract TDTO Entity2DTO(TEntity entity);
        public abstract TEntity DTO2Entity(TDTO dto);

        public virtual IList<TDTO> Entities2DTOs(IList<TEntity> entities)
        {
            return entities.Select(x => Entity2DTO(x)).ToList();
        }

        public virtual IList<TEntity> DTOs2Entities(IList<TDTO> dtos)
        {
            return dtos.Select(x => DTO2Entity(x)).ToList();
        }
    }
}