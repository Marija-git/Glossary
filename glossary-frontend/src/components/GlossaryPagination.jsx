import Pagination from "react-bootstrap/Pagination";
import "bootstrap/dist/css/bootstrap.css";

const GlossaryPagination = () => {
	return (
		<div className='d-flex justify-content-center mt-3'>
			<Pagination>
				<Pagination.Prev />
				<Pagination.Ellipsis />
				<Pagination.Item>{3}</Pagination.Item>
				<Pagination.Item>{4}</Pagination.Item>
				<Pagination.Item>{5}</Pagination.Item>
				<Pagination.Ellipsis />
				<Pagination.Next />
			</Pagination>
		</div>
	);
};

export default GlossaryPagination;
