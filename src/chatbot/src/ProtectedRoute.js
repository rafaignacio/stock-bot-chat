import { useContext } from "react";
import { Route } from "react-router"
import AppContext from "./AppContext";
import LoginComponent from "./components/login/login.component";

const ProtectedRoute = (props) => {
    const ctx = useContext(AppContext);

    if (ctx.token === null) {
        return (<LoginComponent></LoginComponent>);
    }

    return (<Route {...props}></Route>);
};

export default ProtectedRoute;