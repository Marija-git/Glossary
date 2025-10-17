import Pagination from "react-bootstrap/Pagination";
import "bootstrap/dist/css/bootstrap.css";

const GlossaryPagination = ({ pageIndex, totalPages, onPageChange }) => {
	const handleClick = (page) => {
		if (page !== pageIndex) onPageChange(page);
	};

	const paginationItems = [];
	for (let number = 1; number <= totalPages; number++) {
		paginationItems.push(
			<Pagination.Item
				key={number}
				active={number === pageIndex}
				onClick={() => handleClick(number)}>
				{number}
			</Pagination.Item>
		);
	}

	return (
		<div className='d-flex justify-content-center mt-3'>
			<Pagination>
				<Pagination.Prev
					disabled={pageIndex === 1}
					onClick={() => handleClick(pageIndex - 1)}
				/>
				{paginationItems}
				<Pagination.Next
					disabled={pageIndex === totalPages}
					onClick={() => handleClick(pageIndex + 1)}
				/>
			</Pagination>
		</div>
	);
};

export default GlossaryPagination;
