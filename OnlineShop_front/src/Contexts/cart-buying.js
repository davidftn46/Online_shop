import React, { useReducer } from "react";

import CartContext from "./cart-context";

const defaultCartState = {
  products: [],
  totalProducts: 0,
};

const cartReducer = (state, action) => {
  if (action.type === "ADD") {
    const updatedTotalProducts = state.totalProducts + action.product.price * action.product.amount;

    const existingIndex = state.products.findIndex(
      (product) => product.id === action.product.id
    );

    const existingCartProduct = state.products[existingIndex];
    let updatedProducts;
    
    if(existingCartProduct){
      const updatedProduct =
      {
        ...existingCartProduct, 
        amount: existingCartProduct.amount+action.product.amount
      };
      updatedProducts = [...state.products];
      updatedProducts[existingIndex] = updatedProduct;
    }
    else
    {
      updatedProducts= state.products.concat(action.product);
    }

    return {
      products: updatedProducts,
      totalProducts: updatedTotalProducts,
    };
  }

  if(action.type === "REMOVE"){
    
    const existingIndex = state.products.findIndex(
      (product) => product.id === action.id
      );
      
    const existingCartProduct = state.products[existingIndex];
    const updatedTotalProducts = state.totalProducts - existingCartProduct.price;
    let updatedProducts;
    if(existingCartProduct.amount === 1){
      updatedProducts = state.products.filter(product => product.id !== action.id);
    }else{
      const updatedProduct = { ...existingCartProduct , amount: existingCartProduct.amount-1};
      updatedProducts=[...state.products];
      updatedProducts[existingIndex]=updatedProduct;
    }

    return {
      products: updatedProducts,
      totalProducts: updatedTotalProducts
    }
  }
  return defaultCartState;
};

const CartBuying = (props) => {
  const [cartState, dispatchCartAction] = useReducer(
    cartReducer,
    defaultCartState
  );

  const addProductToCartHandler = (product) => {
    dispatchCartAction({ type: "ADD", product: product });
  };

  const removeProductToCarthandler = (id) => {
    dispatchCartAction({ type: "REMOVE", id: id });
  };

  const cartContext = {
    products: cartState.products,
    totalProducts: cartState.totalProducts,
    addProduct: addProductToCartHandler,
    removeProduct: removeProductToCarthandler,
  };

  return (
    <CartContext.Provider value={cartContext}>
      {props.children}
    </CartContext.Provider>
  );
};

export default CartBuying;