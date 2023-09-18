import OrderProduct from './OrderProduct'

class Order {
    constructor(id,comment,address,city,zip) {
      this.UserId=id;
      this.Products = [];
      this.Comment=comment;
      this.Address=address;
      this.City=city;
      this.Zip=zip;
    }
    
    addOrderProduct(id, amount) {
      this.Products=[...this.Products, new OrderProduct(id,amount)];
    }
  }
  
  export default Order;