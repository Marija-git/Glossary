import { Button } from "react-bootstrap";

const ActionsColumn = ({ item, onPublishClick, onDelete, onArchive }) => {
	return (
		<td>
			<Button
				className='me-2'
				onClick={() => onArchive(item)}>
				Archive
			</Button>
			<Button
				className='me-2'
				onClick={() => onDelete(item)}>
				Delete
			</Button>
			<Button onClick={() => onPublishClick(item)}>Publish</Button>
		</td>
	);
};

export default ActionsColumn;
