import React, { useContext, useState } from "react";
import axios from 'axios'

import classes from "./CartDesign.module.css";
import Modal from "../UserInterface/ModalDesign/Modal";
import CartContext from "../../Contexts/cart-context";
import CartProduct from "./CartProduct";
import Order from "../../Models/Order";
import Authentication from '../../Contexts/authentication'
import Input from '../UserInterface/InputDesign/Input'

const Cart = (props) => {
  const ctx = useContext(CartContext);
  const authCtx = useContext(Authentication);

  const [checkedOut, setCheckedOut] = useState(false);
  const [addressIsValid, SetAddressIsValid] = useState(false);
  const [cityIsValid, SetCityIsValid] = useState(false);
  const [zipsIsValid, SetZipIsValid] = useState(false);

  const totalProducts = `$${ctx.totalProducts.toFixed(2)}`;
  const hasProducts = ctx.products.length > 0;

  const cartProductRemoveHandler = (id) => {
    ctx.removeProduct(id)
  };

  const cartProductAddHandler = (product) => {
    ctx.addProduct({ ...product, amount: 1 });
  };

  const cartProducts = (
    <ul className={classes["cart-products"]}>
      {ctx.products.map((product) => (
        <CartProduct
          key={product.id}
          name={product.name}
          amount={product.amount}
          price={product.price}
          onRemove={cartProductRemoveHandler.bind(null, product.id)}
          onAdd={cartProductAddHandler.bind(null, product)}
        />
      ))}
    </ul>
  );

  const OrderHandler = async () => {
    let address = document.getElementById('address');
    let city = document.getElementById('city');
    let zip = document.getElementById('zip');
    let comment = document.getElementById('comment');
    if (!addressIsValid) {
      address.focus();
      return;
    }
    if (!cityIsValid) {
      city.focus();
      return;
    }
    if (!zipsIsValid) {
      zip.focus();
      return;
    }
    const order = new Order(authCtx.user.Id,comment.value,address.value,city.value,zip.value)
    ctx.products.forEach((product) => {
      order.addOrderProduct(product.id, product.amount);
    });

    try {
      const response = await axios.post(process.env.REACT_APP_SERVER_URL + 'orders/newOrder', order, {
        headers: {
          Authorization: `Bearer ${authCtx.user.Token}`
        }
      });

      if (response.data)
        console.log(response.data)

      ctx.emptyCart();
      props.onClose();
    }
    catch (error) {
      console.error(error);
    }

  }

  const checkoutHandler = () => {
    setCheckedOut(!checkedOut);
  }

  const addressChange = () => {
    if (document.getElementById('address').value === "")
      SetAddressIsValid(false);
    else
      SetAddressIsValid(true);
  }
  const cityChange = () => {
    if (document.getElementById('city').value === "")
      SetCityIsValid(false);
    else
      SetCityIsValid(true);
  }
  const zipChange = () => {
    if (document.getElementById('zip').value === "")
      SetZipIsValid(false);
    else
      SetZipIsValid(true);
  }
  const checkout = (
    <div>
      <b><label>Comment:</label></b><br />
      <textarea id='comment' name="comment"></textarea>
      <label style={{color:'red'}}>Fileds with * must not be empty</label>
      <Input id='address' label='Address: *' isValid={addressIsValid} type='text' name='address' onBlur={addressChange}></Input>
      <Input id='city' label='City: *' type="text" name="city" isValid={cityIsValid} onBlur={cityChange}></Input>
      <Input id='zip' label='Zip/Postal code: *' type="text" isValid={zipsIsValid} onBlur={zipChange}></Input>
      <center><button className={classes["button--alt"]} onClick={checkoutHandler}>
        Close
      </button>

        {hasProducts && <button onClick={OrderHandler} className={classes.button}>Order</button>}
      </center>
    </div>
  );


  return (
    <Modal onClose={props.onClose}>
      {checkedOut ? checkout :
        (<>{cartProducts}
          <div className={classes.total}>
            <span>Total Products</span>
            <span>{totalProducts}</span>
          </div>
          <div className={classes.actions}>
            <button className={classes["button--alt"]} onClick={props.onClose}>
              Close
            </button>
            {hasProducts && <button onClick={checkoutHandler} className={classes.button}>Checkout</button>}
          </div></>)}

    </Modal>
  );
};

export default Cart;