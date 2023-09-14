import { useState,useContext } from "react";

import Header from "./Components/Design/Header";
import Cart from "./Components/Cart/Cart";
import Login from "./Components/Authentication/Login";
import Register from './Components/Authentication/Register';
import Authentication from './Contexts/authentication';
import CartBuying from "./Contexts/cart-buying";
import { AuthenticationBuying } from "./Contexts/authentication";
import ProfileInformation from "./Components/Mutual/ProfileInformation";

function App() {

  const ctx= useContext(Authentication);
  const [cartIsShown, setCartIsShown] = useState(false);
  const [LoginIsShown, setLoginIsShown] = useState(false);
  const [RegisterIsShown, setRegisterIsShown] = useState(false);
  const [showProfile, setShowProfile] = useState(false);

  const showProfileHandler = () => {
    setShowProfile(true);
  };

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
    <CartBuying>
      <AuthenticationBuying>
        <div>
        {cartIsShown && <Cart onClose={hideCartHandler}/>}
        {LoginIsShown && <Login onClose={hideLoginFormHandler}/>}
        {RegisterIsShown && <Register onClose={hideRegisterFormHandler}/>}
        <Header onShowProfile={showProfileHandler} onShowCart={showCartHandler}  onShowLoginForm={showLoginFormHandler} onShowRegisterForm={showRegisterFormHandler}/>
        <main>
          {showProfile && <ProfileInformation />}
        </main>
        </div>
      </AuthenticationBuying>
    </CartBuying>
  );
}

export default App;
