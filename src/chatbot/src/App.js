import { Layout } from './components/Layout';
import { Chat } from './components/Chat';
import ProtectedRoute from './ProtectedRoute';
import { useContext, useState } from 'react';
import AppContext from './AppContext';
import { Route } from 'react-router';
import { BrowserRouter, Switch } from 'react-router-dom';
import RegisterComponent from './components/register/register.component';

function App() {
  const appContext = useContext(AppContext);

  let [ token, setToken] = useState(null);

  appContext.token = token;
  appContext.setToken = setToken;

  return (
      <Layout>
        <BrowserRouter>
          <Switch>
            <Route exact path='/' component={Chat} />
            <Route exact path='/register' component={RegisterComponent} />
          </Switch>
        </BrowserRouter>
      </Layout>
      );
  }

export default App;
