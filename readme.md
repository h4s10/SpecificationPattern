# Specification pattern library

.NET Library for implementation of specification pattern, based on EF Core.


# Usage
Specification:

    public class ProductByPlanId : BaseSpecification<Product>  
    {  
      public ProductByPlanId(int planId)  
     {  
	     Criteria = x => x.PlanId == planId;
	     AddInclude(x => x.Include(z => z.Discount).ThenInclude(z => z.Banner));
	     AddOrderBy(x => x.Price);  
     }
    }
Repository:

    public Task<Product?> FirstOrDefaultAsync(ISpecification<Product> spec) =>  
        context.Products.Specify(spec).FirstOrDefaultAsync();
Multiple specifications:

    var whereOnDate = new DiscountWhereOnDate(DateTime.Now);  
    var orderByPriceAsc = new DiscountOrderAscPrice();  
    var byProductIds = new DiscountByProductIds(plans.Select(x => x.Product?.Id));  
    var discounts = await discountRepository.ListAsync(byProductIds && whereOnDate && orderByPriceAsc);
