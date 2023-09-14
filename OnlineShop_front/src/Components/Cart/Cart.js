import React, { useContext } from "react";

import classes from "./CartDesign.module.css";
import Modal from "../UserInterface/ModalDesign/Modal";
import CartContext from "../../Contexts/cart-context";
import CartProduct from "./CartProduct";

const Cart = (props) => {
  const ctx = useContext(CartContext);

  const totalProducts = `$${ctx.totalProducts.toFixed(2)}`;
  const hasProducts = ctx.products.length > 0;

  const cartProductRemoveHandler = (id) => {
    ctx.removeProduct(id)
  };

  const cartProductAddHandler = (product) => {
    ctx.addProduct({...product,amount:1});
  };

  const cartProducts = (
    <ul className={classes["cart-products"]}>
      {ctx.products.map((product) => (
        <CartProduct
          key={product.id}
          name={product.name}
          amount={product.amount}
          price={product.price}
          onRemove={cartProductRemoveHandler.bind(null,product.id)}
          onAdd={cartProductAddHandler.bind(null,product)}
        />
      ))}
    </ul>
  );


  return (
    <Modal onClose={props.onClose}>
    {cartProducts}
    <div className={classes.total}>
      <span>Total Products</span>
      <span>{totalProducts}</span>
    </div>
    <div className={classes.actions}>
      <button className={classes["button--alt"]} onClick={props.onClose}>
        Close
      </button>
      {hasProducts && <button className={classes.button}>Order</button>}
    </div>
  </Modal>
  );
};

export default Cart;