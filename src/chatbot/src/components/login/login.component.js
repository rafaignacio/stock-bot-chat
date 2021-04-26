import "./login.css";
import AppContext from '../../AppContext';
import { useContext, useRef, useState } from "react";

const LoginComponent = () => {
    const ctx = useContext(AppContext);
    const usernameRef = useRef();
    const pwdRef = useRef();

    const [usrErr, setUserError] = useState('');
    const [pwdErr, setPwdError] = useState('');

    return (
        <div className="login-container">
            <div className="username-container">
                <label htmlFor="userName">Username</label>
                <input type="text" id="userName" ref={usernameRef}
                            onChange={handleUsername(setUserError)} data-valid={usrErr === null}/>
                <span>{usrErr}</span>
            </div>
            <div className="password-container">
                <label htmlFor="pwd">Password</label>
                <input type="password" id="pwd" ref={pwdRef}
                            onChange={handlePassword(setPwdError)} data-valid={pwdErr === null}/>
                <span>{pwdErr}</span>
            </div>
            <div className="login-button-container">
                <button onClick={login(ctx, usernameRef, pwdRef)}>Sign In</button>
                <a href="register">Sign Up</a>
            </div>
        </div>
    );
};

const handleUsername = (setErr) => (e) => {
    const { value } = e.target;
    setErr(null);

    if (value.trim().length < 3) {
        setErr('minimum length required: 3');
    }
};

const handlePassword = (setErr) => (e) => {
    const {value} = e.target;
    setErr(null);

    if (value.trim().length < 3) {
        setErr('minimum length required: 3');
    }
};

const login = (ctx, user, pwd) => async (e) => {
    const usrValid = user.current.getAttribute("data-valid");
    const pwdValid = pwd.current.getAttribute("data-valid");

    if (usrValid === 'false' || pwdValid === 'false') {
        alert('check the error messages');
        return;
    }

    const res = await fetch('http://localhost/login');
    
    if (res.status !== 200) {
    }

    if (res.status === 200) {
        ctx.setToken(res.json());
    }
}

export default LoginComponent;