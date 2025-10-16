import { Button } from "react-bootstrap";

const ActionsColumn = ({ item, onPublishClick, onDelete, onArchive }) => {
	const handleDelete = () => {
		if (window.confirm(`Are you sure you want to delete "${item.term}"?`)) {
			onDelete(item);
		}
	};

	const handleArchive = () => {
		if (window.confirm(`Are you sure you want to archive "${item.term}"?`)) {
			onArchive(item);
		}
	};

	return (
		<td>
			<Button
				className='me-2'
				onClick={handleArchive}>
				Archive
			</Button>
			<Button
				className='me-2'
				onClick={handleDelete}>
				Delete
			</Button>
			<Button onClick={() => onPublishClick(item)}>Publish</Button>
		</td>
	);
};

export default ActionsColumn;
