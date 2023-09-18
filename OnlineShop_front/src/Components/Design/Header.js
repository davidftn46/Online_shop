import React, { Fragment, useContext,useState } from 'react';
import { Link } from 'react-router-dom';

import classes from './HeaderDesign.module.css';
import HeaderCartButton from './HeaderCartBtn';
import Authentication from '../../Contexts/authentication';
import Button from '../UserInterface/ButtonDesign/Button';
import { useNavigate } from 'react-router-dom';

const Header = (props) => {
  const ctx = useContext(Authentication);
  const navigate = useNavigate();

  const LogoutHandler = () => {
    navigate('/');
    ctx.onLogout();
  };

  const [dropdownVisible, setDropdownVisible] = useState(false); 

  const toggleDropdown = () => {
    setDropdownVisible(!dropdownVisible);
  };


  return (
    <Fragment>
      <header className={classes.header}>
        <div className={classes.content}>
          <h1>OnlineShop</h1>
              <Link to="/">
                <Button>Home</Button>
              </Link>
          <nav className={classes.nav}>
            <ul>
              {ctx.isLoggedIn ? (
                <>
                  {ctx.user.Role === 1 ? (
                    <Link to="/myOrders">
                      <Button>My orders</Button>
                    </Link>
                  ) : null}
                  {ctx.user.Role === 2 && ctx.user.IsVerified ? 
                  (
                   <div className={classes.dropdown}>
                    <Button onClick={toggleDropdown}>Actions</Button>
                    {dropdownVisible && 
                    (
                      <div className={classes.dropdownContent}>
                        <Link to="/addNew">
                          <button className={classes.dropdownButton}>New item</button>
                        </Link>
                        <Link to="/newOrders">
                        <button className={classes.dropdownButton}>New orders</button>
                        </Link>
                        <Link to="/orderHistory">
                        <button className={classes.dropdownButton}>Order history</button>
                        </Link>
                      </div>
                      )}
                    </div>  
                  ) : null}
                  {ctx.user.Role === 3 ? 
                  (
                   <div className={classes.dropdown}>
                    <Button onClick={toggleDropdown}>Actions</Button>
                    {dropdownVisible && 
                    (
                      <div className={classes.dropdownContent}>
                        <Link to="/allOrders">
                        <button className={classes.dropdownButton}>All orders</button>
                        </Link>
                        <Link to="/verification">
                        <button className={classes.dropdownButton}>New Users</button>
                        </Link>
                      </div>
                      )}
                    </div>  
                  ) : null}
                </>
              ):null}
            </ul>
          </nav>
        </div>
        <div className={classes.content}>
          {ctx.isLoggedIn ? (
            <>
              {ctx.user.Role === 1 ? (
                <HeaderCartButton onClick={props.onShowCart} />
              ) : null}
              <div className={classes.dropdown}>
                <Button onClick={toggleDropdown}>{ctx.user.FirstName} {ctx.user.LastName}</Button>
                  {dropdownVisible && 
                  (
                    <div className={classes.dropdownContent}>
                      <Link to="/profile">
                        <Button onClick={props.onShowProfile}>Profile</Button>
                      </Link>
                      <button className={classes.logout} onClick={LogoutHandler}>Logout</button>
                    </div>
                )}
              </div> 
            </>
          ) : (
            <>
              <Button onClick={props.onShowLoginForm}>Login</Button>
              <Button className onClick={props.onShowRegisterForm}>Register</Button>
            </>
          )}
        </div>
      </header>
    </Fragment>
  );
};

export default Header;