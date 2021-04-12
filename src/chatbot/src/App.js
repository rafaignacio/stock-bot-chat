import { Layout } from './components/Layout';
import { Chat } from './components/Chat';
import { Route } from 'react-router';

function App() {
  return (
    <Layout>
      <Route exact path='/' component={Chat} />
    </Layout>);
  }

export default App;
