import React,{Fragment,useContext} from 'react'

import classes from './HeaderDesign.module.css'
import HeaderCartBtn from './HeaderCartBtn'
import Authentication from '../../Contexts/authentication'
import Button from '../UserInterface/ButtonDesign/Button'

const Header = (props) => {
  const ctx = useContext(Authentication);

  return (
    <Fragment>
        <header className={classes.header}>
            <h1>OnlineShop</h1>
            <nav className={classes.nav}>
              <ul>
                {!ctx.isLoggedIn && (
                    <Button onClick={props.onShowLoginForm}>Login</Button>
             )}
                {!ctx.isLoggedIn && (
                  <Button className onClick={props.onShowRegisterForm}>Register</Button>
                )}
              </ul>
            </nav>
        </header>
    </Fragment>
  )
}

export default Header