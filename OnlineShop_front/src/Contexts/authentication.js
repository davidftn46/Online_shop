import React,{useState, useEffect} from "react";

const Authentication = React.createContext({
    isLoggedIn:false,
    onLoggout: ()=>{},
    onLogin:(email,password)=>{},
    onRegister:()=>{},
});

export const AuthenticationBuying = (props) => {
    const [isLoggedIn,setIsLoggedIn] = useState(false);
    const [user,setUser] = useState({});
    
    useEffect(()=>{
        const storedUserLoggedIn = localStorage.getItem('isLoggedIn');
    
        if(storedUserLoggedIn === '1')
          setIsLoggedIn(true)
    
    }, []);

    const logoutHandler =()=>{
        localStorage.removeItem('isLoggedIn');
        setIsLoggedIn(false);
    }

    const loginHandler=(user)=>{
        localStorage.setItem('token', user.Token);
        localStorage.setItem('isLoggedIn','1');
        setIsLoggedIn(true);
        setUser(user);
    }

    const RegisterHandler=()=>{
        localStorage.setItem('isLoggedIn','1');
        setIsLoggedIn(true);
    }

    return (
        <Authentication.Provider
        value={{isLoggedIn: isLoggedIn,
            user:user,
            onLogout: logoutHandler, 
            onLogin: loginHandler, 
            onRegister:RegisterHandler}}>
            {props.children}
        </Authentication.Provider>
    )

}

export default Authentication;