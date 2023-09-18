import { useState } from "react";
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';

import Header from "./Components/Design/Header";
import Cart from "./Components/Cart/Cart";
import Login from "./Components/Authentication/Login";
import Register from './Components/Authentication/Register'
import CartBuying from "./Contexts/cart-buying";
import { AuthenticationBuying } from "./Contexts/authentication";
import { ProductContextBuying } from "./Contexts/product-context";
import { OrderContextBuying } from "./Contexts/order-context";
import ProfileInformation from "./Components/Mutual/ProfileInformation";
import Dashboard from "./Components/Home/Dashboard";
//Customer
import MyOrders from "./Components/Mutual/CustomerComponents/MyOrders";
// Seller
import NewProduct from './Components/Mutual/SellerComponents/NewProduct'
import NewOrders from './Components/Mutual/SellerComponents/NewOrders'
import OrderHistory from './Components/Mutual/SellerComponents/OrderHistory'
//Administrator
import AllOrders from './Components/Mutual/AdminComponents/AllOrders'
import Verification from './Components/Mutual/AdminComponents/Verification'

function App() {
  const [cartIsShown, setCartIsShown] = useState(false);
  const [LoginIsShown, setLoginIsShown] = useState(false);
  const [RegisterIsShown, setRegisterIsShown] = useState(false);

  const showCartHandler = () => {
    setCartIsShown(true);
  };

  const hideCartHandler = () => {
    setCartIsShown(false);
  };

  const showLoginFormHandler = () => {
    setLoginIsShown(true);
  };

  const hideLoginFormHandler = () => {
    setLoginIsShown(false);
  };

  const showRegisterFormHandler = () => {
    setRegisterIsShown(true);
  };

  const hideRegisterFormHandler = () => {
    setRegisterIsShown(false);
  };

  return (
    <AuthenticationBuying>
      <ProductContextBuying>
        <OrderContextBuying>
        <CartBuying>
      <Router>
        {cartIsShown && <Cart onClose={hideCartHandler}/>}
        {LoginIsShown && <Login onClose={hideLoginFormHandler}/>}
        {RegisterIsShown && <Register onClose={hideRegisterFormHandler}/>}
        <Header onShowCart={showCartHandler}  onShowLoginForm={showLoginFormHandler} onShowRegisterForm={showRegisterFormHandler}/>
        <main>
          <Routes>
            <Route path="/" exact element={<Dashboard/>} />
            <Route path="/profile" element={<ProfileInformation />} />
            <Route path="/myOrders" element={<MyOrders />} />
            <Route path="/orderHistory" element={<OrderHistory />} />
            <Route path="/addNew" element={<NewProduct />} />
            <Route path="/myOrders" element={<MyOrders />} />
            <Route path="/newOrders" element={<NewOrders />} />
            <Route path="/verification" element={<Verification />} />
            <Route path="/allOrders" element={<AllOrders />} />
        </Routes>
        </main>
        </Router>
          </CartBuying>
          </OrderContextBuying>
        </ProductContextBuying>
    </AuthenticationBuying>
  );
}

export default App;