import { createContext } from "react";

const AppContext = createContext({
    token: null,
    setToken: null,
});

export default AppContext;