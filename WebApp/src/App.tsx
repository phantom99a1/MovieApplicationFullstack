import { Container, Nav, Navbar } from "react-bootstrap";
import { BrowserRouter, Link, Route } from "react-router-dom";
import Landing from "./pages/Landing";
import Actors from "./pages/Actors";

const App = () => {
  return (
    <Container>
		<BrowserRouter>
			<Navbar bg="dark" variant="dark">
				<Navbar.Brand as={Link} to="/">Movie World</Navbar.Brand>
				<Nav className="mr-auto">
					<Nav.Link as={Link} to="/">Movies</Nav.Link>
					<Nav.Link as={Link} to="/actors">Actors</Nav.Link>
				</Nav>
			</Navbar>
			<switch>
				<Route path="/" Component={() => <Landing />}/>
				<Route path="/actors" Component={() => <Actors />}/>
			</switch>
		</BrowserRouter>
    </Container>
  );
};

export default App;
