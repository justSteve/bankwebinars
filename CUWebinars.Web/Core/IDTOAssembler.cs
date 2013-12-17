using System.Collections.Generic;

namespace CUWebinars.Web.Core
{
    public interface IDTOAssembler<TDTO, TEntity>
    {
        TDTO Entity2DTO(TEntity entity);
        TEntity DTO2Entity(TDTO dto);
        IList<TDTO> Entities2DTOs(IList<TEntity> entities);
        IList<TEntity> DTOs2Entities(IList<TDTO> dtos);
    }
}